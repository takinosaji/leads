import sys
import shutil
import os
from pathlib import Path

# Default build output location and binary name
REPO_ROOT = Path(__file__).resolve().parents[1]
DIST_DIR = REPO_ROOT / "installer" / "dist"
DEFAULT_BINARY_NAME = "leads"

# macOS user bin directory (~/bin is common, but /usr/local/bin is standard for user-wide CLI tools)
USER_BIN_DIRS = [
    Path.home() / "bin",
    Path("/usr/local/bin"),
]

def find_cli_binary(dist_dir: Path, binary_name: str) -> Path:
    bin_path = dist_dir / binary_name
    if bin_path.exists():
        return bin_path
    # Try with .app or .exe extensions if needed
    for ext in ("", ".app", ".exe"):
        candidate = dist_dir / f"{binary_name}{ext}"
        if candidate.exists():
            return candidate
    raise FileNotFoundError(f"Built CLI binary not found in {dist_dir} (looked for {binary_name})")

def get_target_bin_dir() -> Path:
    # Prefer ~/bin if it exists or can be created, else fallback to /usr/local/bin
    for d in USER_BIN_DIRS:
        try:
            d.mkdir(parents=True, exist_ok=True)
            if os.access(str(d), os.W_OK):
                return d
        except Exception:
            continue
    raise PermissionError("No writable user bin directory found (tried ~/bin and /usr/local/bin)")

def install_cli_to_user_bin():
    if sys.platform != "darwin":
        print("This script is intended for macOS only.")
        sys.exit(1)
    try:
        cli_bin = find_cli_binary(DIST_DIR, DEFAULT_BINARY_NAME)
        target_dir = get_target_bin_dir()
        target_path = target_dir / DEFAULT_BINARY_NAME
        shutil.copy2(cli_bin, target_path)
        target_path.chmod(0o755)
        print(f"Installed {cli_bin} to {target_path}")
    except Exception as e:
        print(f"Failed to install CLI: {e}")
        sys.exit(1)

if __name__ == "__main__":
    install_cli_to_user_bin()
