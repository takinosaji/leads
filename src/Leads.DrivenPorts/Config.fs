namespace Leads.DrivenPorts.Config

module DTO = 
    type ConfigDrivenOutputDto = Map<string, string> Option
    
    type ValidConfigDrivenInputDto = (string * string) list option 
    module ValidConfigDrivenInputDto =
        let find (validConfiguration: ValidConfigDrivenInputDto) (key: string) =
            match validConfiguration with
            | Some configuration  ->
               match List.tryFind (fun li -> fst(li) = key) configuration with
               | Some configEntry ->
                   snd configEntry |> Some
               | None -> None
            | None -> None
            
        let findOrDefault (validConfiguration: ValidConfigDrivenInputDto) (key: string) (``default``: string) =
            match find validConfiguration key with
            | Some value -> value
            | None -> ``default``               
open DTO    

type ConfigurationProvider = unit -> Result<ConfigDrivenOutputDto, string>
type ConfigurationValueApplier = string -> string -> Result<unit, string>