from pathlib import Path
import tempfile
import unittest
import zipfile

import release


class ReleaseTests(unittest.TestCase):
    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.project = Path(directory.name) / "ConfigurableQuestsLimit.csproj"
        self.project.write_text("<Project><PropertyGroup><Version>1.0.1</Version></PropertyGroup></Project>", encoding="utf-8")

    def test_bepinex_tag_matches_project_version(self):
        self.assertEqual(release.validate_project_version(self.project, "bepinex-1.0.1"), "1.0.1")

    def test_melonloader_tag_is_rejected(self):
        with self.assertRaises(ValueError):
            release.validate_project_version(self.project, "1.0.1")

    def test_different_bepinex_version_is_rejected(self):
        with self.assertRaises(ValueError):
            release.validate_project_version(self.project, "bepinex-1.0.2")

    def test_package_contains_only_the_plugin(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            source = root / "bin/Release/netstandard2.1/ConfigurableQuestsLimit.dll"
            source.parent.mkdir(parents=True)
            source.write_bytes(b"plugin")
            source.with_name("BepInEx.dll").write_bytes(b"loader")
            archive = release.package(root, "1.0.1", root / "dist")
            self.assertEqual(archive.name, "ConfigurableQuestsLimit-BepInEx-1.0.1.zip")
            with zipfile.ZipFile(archive) as zipped:
                self.assertEqual(zipped.namelist(), [source.name])
                self.assertEqual(zipped.read(source.name), b"plugin")

    def test_missing_plugin_is_rejected(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            with self.assertRaises(FileNotFoundError):
                release.package(root, "1.0.1", root / "dist")


if __name__ == "__main__":
    unittest.main()
