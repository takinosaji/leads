// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"




leads config set config-name config-value
leags config get config-name

leads add stream --name
leads add trail --theme theme-text
leads add lead --trail hash exact match OR THEME LIKE --text text --tags tags


leads use --name stream-name LIKE

leads delete stream --name
leads delete trail --hash --theme
leads delete lead --hash --text

leads describe stream
leads describe trail 
leads describe lead hash exact match OR text LIKE
leads describe tags --tags tag-or-tags --latest # THis commang gets leads marked with specific tags sorted chronologically. Each item is lead with reference to trail, date time etc

leads list streams
leads list trails [--stream]

leads list leads --trail hash exact match OR THEME LIKE
SAME AS
leads bla bla bla --latest

leads list tags # List all tags

leads link --source-hash lead-hash --target-hash target-hash --target-theme