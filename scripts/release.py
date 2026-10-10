"""Validate release metadata and package the mod using only the standard library."""

import argparse
import os
from pathlib import Path
import re
import sys
import uuid
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[1]
VERSION = re.compile(r"((?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*))\Z")


def normalize_version(value):
    match = VERSION.fullmatch(value)
    if not match or any(int(part) > 65534 for part in match[1].split(".")):
        raise ValueError(f"Expected a stable version such as 1.0.1 without a prefix, got {value!r}")
    return match[1]


def validate_project_version(project, tag=None):
    version = ET.parse(project).findtext("./PropertyGroup/Version")
    if not version:
        raise ValueError("The project must define a Version property")
    version = normalize_version(version)
    if tag is not None and normalize_version(tag) != version:
        raise ValueError(f"Tag {tag!r} does not match project Version {version!r}")
    return version


def extract_changelog(text, version):
    lines = text.splitlines()
    headings = []
    fence = None
    for index, line in enumerate(lines):
        marker = re.match(r"^\s{0,3}(`{3,}|~{3,})", line)
        if marker:
            value = marker[1]
            if fence is None:
                fence = value
            elif value[0] == fence[0] and len(value) >= len(fence):
                fence = None
            continue
        if fence is None:
            heading = re.match(r"^(#{1,2})\s+(.+?)\s*#*\s*$", line)
            if heading:
                headings.append((index, len(heading[1]), heading[2]))
    matches = [heading for heading in headings if heading[2] == version]
    if len(matches) != 1:
        raise ValueError(f"Expected exactly one changelog section for {version}, found {len(matches)}")
    start, level, _ = matches[0]
    end = next((index for index, depth, _ in headings if index > start and depth <= level), len(lines))
    notes = "\n".join(lines[start + 1:end]).strip()
    if not notes:
        raise ValueError(f"Changelog section for {version} is empty")
    return notes


def github_output(name, value):
    if os.environ.get("GITHUB_OUTPUT"):
        delimiter = f"OUTPUT_{uuid.uuid4().hex}"
        with open(os.environ["GITHUB_OUTPUT"], "a", encoding="utf-8") as output:
            output.write(f"{name}<<{delimiter}\n{value}\n{delimiter}\n")


def metadata(root, tag, destination):
    version = validate_project_version(root / "ConfigurableQuestsLimit.csproj", tag)
    notes = extract_changelog((root / "CHANGELOG.md").read_text(encoding="utf-8-sig"), version)
    destination.mkdir(parents=True, exist_ok=True)
    (destination / "release-notes.md").write_text(notes + "\n", encoding="utf-8")
    github_output("version", version)
    github_output("changelog", notes)
    print(f"Validated release {version}")


def package(root, version, destination):
    version = normalize_version(version)
    source = root / "bin/Release/netstandard2.1/ConfigurableQuestsLimit.dll"
    if not source.is_file():
        raise FileNotFoundError(f"Required release file is missing: {source}")
    destination.mkdir(parents=True, exist_ok=True)
    archive = destination / f"ConfigurableQuestsLimit-{version}.zip"
    with zipfile.ZipFile(archive, "w", compression=zipfile.ZIP_DEFLATED) as zipped:
        zipped.write(source, source.name)
    print(f"Created {archive}")
    print(f"Archive contains only {source.name}")
    return archive


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("command", choices=("metadata", "package"))
    parser.add_argument("--tag")
    parser.add_argument("--output", type=Path, default=ROOT / "dist")
    args = parser.parse_args()
    try:
        if args.command == "metadata":
            metadata(ROOT, args.tag, args.output)
        else:
            version = validate_project_version(ROOT / "ConfigurableQuestsLimit.csproj", args.tag)
            package(ROOT, version, args.output)
    except (ValueError, OSError, ET.ParseError) as error:
        print(f"Release error: {error}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
