from textual.markup import escape


def escape_textual(text: str) -> str:
    """Escape text so it is safe to render in Textual/Rich markup.

    This wraps `textual.markup.escape` and additionally replaces raw
    angle brackets. It's meant for exception messages and tracebacks
    that may include characters which confuse the markup parser.
    """
    if text is None:
        return ""

    escaped = escape(str(text))
    # Extra safety: normalise any remaining angle brackets so they
    # can never be parsed as markup tags.
    return escaped.replace("<", "[").replace(">", "]")
