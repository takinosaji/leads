configs:
default.forest
working.dir

leads config set config-name config-value
leads config get config-name

leads forest list --status

leads add forest [name]
leads add trail [theme-text] --forest [forest-hash exact match or forest-name LIKE]
leads add lead [text] --trail [trail-hash exact match or trail-theme LIKE] --tags [tags] --new


leads use [forest-hash exact match or forest-name LIKE]

leads delete forest [forest-hash exact match or forest-name LIKE]
leads delete trail [trail-hash exact match or trail-theme LIKE]
leads delete lead [lead-hash exact match or lead-text LIKE]

leads move trail [trail-hash exact match or trail-theme LIKE] --source-forest [forest-hash exact match or forest-name LIKE] --target-forest [forest-hash exact match or forest-name LIKE]

leads copy trail [trail-hash exact match or trail-theme LIKE] --source-forest [forest-hash exact match or forest-name LIKE] --target-forest [forest-hash exact match or forest-name LIKE]

leads describe forest
leads describe trail
leads describe lead hash exact match OR text LIKE
leads describe tags --tags tag-or-tags --latest # THis command gets leads marked with specific tags sorted chronologically. Each item is lead with reference to trail, date time etc


leads list trails [--forest]

leads list leads trail-hash exact match OR THEME LIKE
SAME AS
leads bla bla bla --latest

leads list tags # List all tags

leads link --source-hash lead-hash --target-hash target-hash --target-theme
leads unlink

leads complete trail-hash exact match OR THEME LIKE