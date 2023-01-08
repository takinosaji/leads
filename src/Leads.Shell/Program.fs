// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"


configs:
default.stream
working.dir

leads config set config-name config-value
leags config get config-name

leads add stream [name]
leads add trail [theme-text] --stream [stream-hash exact match or stream-name LIKE]
leads add lead --trail [trail-hash exact match or trail-theme LIKE] --tags [tags] [text]


leads use [stream-hash exact match or stream-name LIKE]

leads delete stream [stream-hash exact match or stream-name LIKE]
leads delete trail [trail-hash exact match or trail-theme LIKE]
leads delete lead [lead-hash exact match or lead-text LIKE]

leads move trail [trail-hash exact match or trail-theme LIKE] --source-stream [stream-hash exact match or stream-name LIKE] --target-stream [stream-hash exact match or stream-name LIKE]

leads copy trail [trail-hash exact match or trail-theme LIKE] --source-stream [stream-hash exact match or stream-name LIKE] --target-stream [stream-hash exact match or stream-name LIKE]

leads describe stream
leads describe trail 
leads describe lead hash exact match OR text LIKE
leads describe tags --tags tag-or-tags --latest # THis commang gets leads marked with specific tags sorted chronologically. Each item is lead with reference to trail, date time etc

leads list streams
leads list trails [--stream]

leads list leads trail-hash exact match OR THEME LIKE
SAME AS
leads bla bla bla --latest

leads list tags # List all tags

leads link --source-hash lead-hash --target-hash target-hash --target-theme
leads unlink

leads complete trail-hash exact match OR THEME LIKE