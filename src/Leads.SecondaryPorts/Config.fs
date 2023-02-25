namespace Leads.SecondaryPorts.Config

module DTO = 
    type ConfigSecondaryOutputDto = Map<string, string> Option
    
    type ValidConfigSIDto = (string * string) list option 
    module ValidConfigSIDto =
        let find (validConfiguration: ValidConfigSIDto) (key: string) =
            match validConfiguration with
            | Some configuration  ->
               match List.tryFind (fun li -> fst(li) = key) configuration with
               | Some configEntry ->
                   snd configEntry |> Some
               | None -> None
            | None -> None
            
        let findOrDefault (validConfiguration: ValidConfigSIDto) (key: string) (``default``: string) =
            match find validConfiguration key with
            | Some value -> value
            | None -> ``default``               
open DTO    

type ConfigurationProvider = unit -> Result<ConfigSecondaryOutputDto, string>
type ConfigurationValueApplier = string -> string -> Result<unit, string>