from prompt_toolkit.styles import Style


STYLE_DICT = {
    # ASCII-art LEADS header: dark background with bright orange text.
    "header-ascii": "bg:#202020 #ffaf00 bold",

    # Header: blue bar with white, bold text (k9s-like top banner).
    "header": "bg:#202020 #ffffff bold",

    # Frame borders/labels: blue borders and labels.
    "frame.border": "fg:#268bd2",
    "frame.label": "bg:#202020 #268bd2 bold",

    # Menu: dark panel with orange menu item text.
    "menu": "bg:#202020 #d0d0d0",
    # Menu items: orange text, inherit border color from frame.border (blue).
    "menu-item": "bg:#202020 #888888",
    "menu-selected-active": "bg:#268bd2 #ffaf00 bold",
    "menu-selected-inactive": "bg:#202020 #ffaf00",

    # Content area: near-black background with light gray text.
    "content": "bg:#000000 #d0d0d0",
    "content-title": "#ffffff bold",
    "content-text": "#d0d0d0",
    "content-selected-active": "bg:#268bd2 #ffffff",
    "content-selected-inactive": "reverse",

    # Status bar: subtle bottom bar using dark background and light text.
    "status-bar": "bg:#202020 #d0d0d0",

    # Notification panel styles
    "notification-bar": "bg:#202020 #d0d0d0",
    "notification-empty": "bg:#202020 #d0d0d0",
    "notification-error": "bg:#440000 #ff4444",
    "notification-success": "bg:#004400 #44ff44",

    # Title panel styles
    "title-key": "#ffaf00 bold",
    "title-value": "#ffffff",

    # Command panel style
    "command-bar": "bg:#000000 #d0d0d0",
}


def build_style() -> Style:
    return Style.from_dict(STYLE_DICT)
