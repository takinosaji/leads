module Leads.SecondaryAdapters.JsonFileBased.TrailAdapters

open System.IO
open Leads.SecondaryPorts.Config.DTO

open Leads.SecondaryAdapters.JsonFileBased.AllowedKeys

let private getTrailFolderPath
    (defaultWorkingDirPath: string)
    (validConfigurationDto: ValidConfigSIDto)
    streamHash = 
    let workingDirPath = ValidConfigSIDto.findOrDefault
                            validConfigurationDto
                            WorkingDirKey
                            defaultWorkingDirPath
    Path.Combine(workingDirPath, streamHash)
    
let createLocalJsonFileTrailAdapters defaultWorkingDirPath =
    {|
        addTrail = fun validConfigurationDto trailDto ->
           addTrail <| getTrailFolderPath defaultWorkingDirPath validConfigurationDto <| trailDto           
    |}