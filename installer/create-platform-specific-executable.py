import subprocess
import argparse
import sys
from pathlib import Path
from typing import List, Optional

REPO_ROOT = Path(__file__).resolve().parents[1]
VENV_PACKAGES_PATH = REPO_ROOT / ".venv/Lib/site-packages"
APP_ROOT = REPO_ROOT / "app"
CLI_ENTRY_DEFAULT = APP_ROOT / "leads" / "cli" / "__main__.py"
DEFAULT_DIST_DIR = REPO_ROOT / "installer" / "dist"
DEFAULT_BUILD_DIR = REPO_ROOT / "installer" / "build"
DEFAULT_NAME = "leads"

# Collect module search paths under app/leads to ensure PyInstaller can resolve imports.
DEFAULT_MODULE_DIRS = [
    APP_ROOT,
    VENV_PACKAGES_PATH
]


def ensure_pyinstaller_available() -> None:
    try:
        subprocess.check_call(["pyinstaller", "--version"], cwd=str(REPO_ROOT))
    except subprocess.CalledProcessError as e:
        print(f"PyInstaller check failed. Error: {e}")
        raise e
    except FileNotFoundError:
        print("PyInstaller is not installed. Install it with: pip install pyinstaller")
        raise


# --- Platform metadata helpers -------------------------------------------------

def write_windows_version_file(
    build_dir: Path,
    product_name: str,
    version: str,
    company: Optional[str] = None,
    copyright_text: Optional[str] = None,
) -> Path:
    """Create a Windows version resource file for PyInstaller's --version-file.

    Version must be in the form 'MAJOR.MINOR.PATCH[.BUILD]'.
    """
    vf = build_dir / f"{product_name}-version-info.txt"
    company = company or product_name
    copyright_text = copyright_text or f"Copyright (C) {company}"

    # Map 1.2.3.4 to tuple of ints for FileVersion/ProductVersion
    def dotted_to_tuple(v: str) -> str:
        parts = v.split(".")
        nums = [int(p) if p.isdigit() else 0 for p in parts]
        while len(nums) < 4:
            nums.append(0)
        return ", ".join(str(n) for n in nums[:4])

    file_version_tuple = dotted_to_tuple(version)

    content = f"""
# UTF-8
VSVersionInfo(
  ffi=FixedFileInfo(
    filevers=({file_version_tuple}),
    prodvers=({file_version_tuple}),
    mask=0x3f,
    flags=0x0,
    OS=0x40004,
    fileType=0x1,
    subtype=0x0,
    date=(0, 0)
  ),
  kids=[
    StringFileInfo([
      StringTable(
        u'040904B0',
        [
          StringStruct(u'CompanyName', u'{company}'),
          StringStruct(u'FileDescription', u'{product_name}'),
          StringStruct(u'FileVersion', u'{version}'),
          StringStruct(u'InternalName', u'{product_name}'),
          StringStruct(u'LegalCopyright', u'{copyright_text}'),
          StringStruct(u'OriginalFilename', u'{product_name}.exe'),
          StringStruct(u'ProductName', u'{product_name}'),
          StringStruct(u'ProductVersion', u'{version}')
        ]
      )
    ]),
    VarFileInfo([
      VarStruct(u'Translation', [0x0409, 0x04B0])
    ])
  ]
)
""".strip()

    vf.write_text(content, encoding="utf-8")
    return vf


def write_macos_plist(
    build_dir: Path,
    product_name: str,
    version: str,
    bundle_id: str,
) -> Path:
    """Create a minimal Info.plist additions file for macOS to set version fields.
    This file can be passed via --plist to PyInstaller.
    """
    plist_path = build_dir / f"{product_name}-Info.plist"
    content = f"""
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
  <dict>
    <key>CFBundleIdentifier</key>
    <string>{bundle_id}</string>
    <key>CFBundleName</key>
    <string>{product_name}</string>
    <key>CFBundleShortVersionString</key>
    <string>{version}</string>
    <key>CFBundleVersion</key>
    <string>{version}</string>
  </dict>
</plist>
""".strip()
    plist_path.write_text(content, encoding="utf-8")
    return plist_path


# Read version fallback from app/leads/cli/VERSION.txt
CLI_VERSION_FILE = APP_ROOT / "leads" / "cli" / "VERSION.txt"

def read_version_from_cli_file() -> Optional[str]:
    try:
        if CLI_VERSION_FILE.exists():
            v = CLI_VERSION_FILE.read_text(encoding="utf-8").strip()
            return v or None
    except Exception as e:
        print(f"Warning: failed to read version from {CLI_VERSION_FILE}: {e}")
    return None


def build_pyinstaller_command(
    entry: Path,
    name: str,
    dist_dir: Path,
    build_dir: Path,
    include_paths: List[Path],
    windowed: bool,
    clean: bool,
    single_file: bool,
    osx_bundle_id: Optional[str] = None,
    add_data: Optional[List[str]] = None,
    add_binary: Optional[List[str]] = None,
    version: Optional[str] = None,
    company: Optional[str] = None,
    copyright_text: Optional[str] = None,
) -> List[str]:
    command = [
        "pyinstaller",
    ]

    # Control single-file (-F) vs one-folder (default)
    if single_file:
        command.append("-F")

    command.extend([
        "-n", name,
        "--distpath", str(dist_dir),
        "--workpath", str(build_dir),
        "--specpath", str(build_dir),
    ])

    if windowed:
        command.append("-w")

    if clean:
        command.append("--clean")

    for p in include_paths:
        if p.exists():
            command.extend(["--paths", str(p)])

    # Platform-specific metadata flags
    platform = sys.platform
    if platform == "darwin" and osx_bundle_id:
        command.extend(["--osx-bundle-identifier", osx_bundle_id])
        # If version provided, generate a plist snippet to inject version fields
        if version:
            plist_path = write_macos_plist(build_dir, name, version, osx_bundle_id)
            command.extend(["--plist", str(plist_path)])

    # Windows version resource
    if platform.startswith("win") and version:
        vf_path = write_windows_version_file(build_dir, name, version, company, copyright_text)
        command.extend(["--version-file", str(vf_path)])

    # Pass through any user-specified add-data/add-binary
    if add_data:
        for d in add_data:
            command.extend(["--add-data", d])

    if add_binary:
        for b in add_binary:
            command.extend(["--add-binary", b])

    # For Linux: there is no standard executable metadata; optionally embed VERSION.txt
    if platform.startswith("linux") and version:
        version_file = build_dir / f"{name}-VERSION.txt"
        try:
            version_file.write_text(version, encoding="utf-8")
            # Use ':' on Unix separators for --add-data
            command.extend(["--add-data", f"{version_file}:{name}"])
        except Exception:
            pass

    command.append(str(entry))
    return command


def create_single_file_executable(
    entry: str,
    name: str,
    dist_dir: str,
    build_dir: str,
    module_dirs: Optional[List[str]] = None,
    windowed: bool = False,
    no_clean: bool = False,
    single_file: bool = True,
    osx_bundle_id: Optional[str] = None,
    add_data: Optional[List[str]] = None,
    add_binary: Optional[List[str]] = None,
    version: Optional[str] = None,
    company: Optional[str] = None,
    copyright_text: Optional[str] = None,
):
    entry_path = Path(entry).resolve()
    dist_path = Path(dist_dir).resolve()
    build_path = Path(build_dir).resolve()

    if not entry_path.exists():
        print(f"Entry point not found: {entry_path}")
        return

    dist_path.mkdir(parents=True, exist_ok=True)
    build_path.mkdir(parents=True, exist_ok=True)

    include_paths = DEFAULT_MODULE_DIRS if module_dirs is None else [Path(p).resolve() for p in module_dirs]

    # Clean previous contents unless --no-clean is set
    clean = not no_clean
    if clean:
        for target in (dist_path, build_path):
            for item in target.glob("*"):
                try:
                    if item.is_dir():
                        subprocess.call(["powershell", "-NoProfile", "-Command", f"Remove-Item -Recurse -Force '{item}'"], cwd=str(REPO_ROOT))
                    else:
                        item.unlink(missing_ok=True)
                except Exception as e:
                    print(f"Warning: failed to clean {item}: {e}")

    ensure_pyinstaller_available()

    # Fallback to VERSION.txt if --version empty or not provided
    normalized_version = (version or '').strip() or read_version_from_cli_file()
    if normalized_version:
        print(f"Using version: {normalized_version}")
    else:
        print("No version provided and VERSION.txt not found or empty; proceeding without platform metadata versioning.")

    command = build_pyinstaller_command(
        entry=entry_path,
        name=name,
        dist_dir=dist_path,
        build_dir=build_path,
        include_paths=include_paths,
        windowed=windowed,
        clean=clean,
        single_file=single_file,
        osx_bundle_id=osx_bundle_id,
        add_data=add_data,
        add_binary=add_binary,
        version=normalized_version,
        company=company,
        copyright_text=copyright_text,
    )

    try:
        print("Building executable with PyInstaller...")
        print("Command:")
        print(" ", " ".join(command))
        subprocess.check_call(command, cwd=str(REPO_ROOT))
        print(f"Successfully built artifact(s) in {dist_path}.")
    except subprocess.CalledProcessError as e:
        print(f"Failed to build executable. Error: {e}")
        raise e


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description="Create an executable for the leads CLI using PyInstaller.")
    parser.add_argument('--entry', type=str, default=str(CLI_ENTRY_DEFAULT), help="Path to the entrypoint Python file.")
    parser.add_argument('--name', type=str, default=DEFAULT_NAME, help="Name of the resulting executable.")
    parser.add_argument('--dist', type=str, default=str(DEFAULT_DIST_DIR), help="Output dist directory.")
    parser.add_argument('--build', type=str, default=str(DEFAULT_BUILD_DIR), help="Build/work directory.")
    parser.add_argument('--module-dirs', nargs='*', help="Override module directories to include via --paths.")
    parser.add_argument('--windowed', action='store_true', help="Build windowed app (no console).")
    parser.add_argument('--no-clean', action='store_true', help="Do not clean previous build artifacts.")
    parser.add_argument('--single-file', dest='single_file', action='store_true', default=True, help="Build a single-file executable (-F). Default: true.")
    parser.add_argument('--no-single-file', dest='single_file', action='store_false', help="Build one-folder instead of single-file.")
    parser.add_argument('--osx-bundle-id', type=str, default="leads", help="macOS: bundle identifier (e.g., com.example.leads).")
    parser.add_argument('--add-data', nargs='*', help="Additional --add-data entries (format: SRC;DEST on Windows, SRC:DEST on Unix/macOS)")
    parser.add_argument('--add-binary', nargs='*', help="Additional --add-binary entries (format: SRC;DEST or SRC:DEST)")
    # Versioning metadata
    parser.add_argument('--version', type=str, help="Product/app version (e.g., 1.2.3 or 1.2.3.4). Injected into platform-specific metadata where supported.")
    parser.add_argument('--company', type=str, help="Company/author name for metadata (Windows resource).")
    parser.add_argument('--copyright', dest='copyright_text', type=str, help="Copyright text for metadata (Windows resource).")
    args = parser.parse_args()

    create_single_file_executable(
        entry=args.entry,
        name=args.name,
        dist_dir=args.dist,
        build_dir=args.build,
        module_dirs=args.module_dirs,
        windowed=args.windowed,
        no_clean=args.no_clean,
        single_file=args.single_file,
        osx_bundle_id=args.osx_bundle_id,
        add_data=args.add_data,
        add_binary=args.add_binary,
        version=args.version,
        company=args.company,
        copyright_text=args.copyright_text,
    )
