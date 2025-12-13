from prompt_toolkit.styles import Style


STYLE_DICT = {
    # ASCII-art LEADS header: dark background with bright orange text.
    "header-ascii": "bg:#000000 #ffaf00 bold",

    # Header: blue bar with white, bold text (k9s-like top banner).
    "header": "bg:#1e3a5f #ffffff bold",

    # Frame borders/labels: blue borders and labels.
    "frame.border": "fg:#268bd2",
    "frame.label": "bg:#000000 #268bd2 bold",

    # Menu: dark panel with orange menu item text.
    "menu": "bg:#101318 #d0d0d0",
    # Menu items: orange text, inherit border color from frame.border (blue).
    "menu-item": "bg:#101318 #ffaf00",
    "menu-selected": "bg:#268bd2 #ffaf00 bold",

    # Content area: near-black background with light gray text.
    "content": "bg:#000000 #d0d0d0",
    "content-title": "#ffffff bold",
    "content-text": "#d0d0d0",

    # Status bar: subtle bottom bar using dark background and light text.
    "status-bar": "bg:#101318 #d0d0d0",
}


def build_style() -> Style:
    return Style.from_dict(STYLE_DICT)
