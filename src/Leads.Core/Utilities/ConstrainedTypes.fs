module Leads.Core.Utilities.ConstrainedTypes

open System

let createNotEmptyString fieldName ctor str = 
    if String.IsNullOrEmpty(str) then
        let msg = $"%s{fieldName} must not be null or empty" 
        Error msg
    else
        Ok (ctor str)
            

let createLimitedString fieldName ctor maxLen str = 
    if String.IsNullOrEmpty(str) then
        let msg = $"%s{fieldName} must not be null or empty" 
        Error msg
    elif str.Length > maxLen then
        let msg = $"%s{fieldName} must not be more than %i{maxLen} chars" 
        Error msg 
    else
        Ok (ctor str)


let createStringOption fieldName ctor maxLen str = 
    if String.IsNullOrEmpty(str) then
        Ok None
    elif str.Length > maxLen then
        let msg = $"%s{fieldName} must not be more than %i{maxLen} chars" 
        Error msg 
    else
        Ok (ctor str |> Some)


let createInt fieldName ctor minVal maxVal i = 
    if i < minVal then
        let msg = $"%s{fieldName}: Must not be less than %i{minVal}"
        Error msg
    elif i > maxVal then
        let msg = $"%s{fieldName}: Must not be greater than %i{maxVal}"
        Error msg
    else
        Ok (ctor i)

let createFloat fieldName ctor minVal maxVal i = 
    if i < minVal then
        let msg = $"%s{fieldName}: Must not be less than %f{minVal}"
        Error msg
    elif i > maxVal then
        let msg = $"%s{fieldName}: Must not be greater than %f{maxVal}"
        Error msg
    else
        Ok (ctor i)

let createDecimal fieldName ctor minVal maxVal i = 
    if i < minVal then
        let msg = $"%s{fieldName}: Must not be less than %M{minVal}"
        Error msg
    elif i > maxVal then
        let msg = $"%s{fieldName}: Must not be greater than %M{maxVal}"
        Error msg
    else
        Ok (ctor i)

let createLike fieldName ctor pattern str = 
    if String.IsNullOrEmpty(str) then
        let msg = $"%s{fieldName}: Must not be null or empty" 
        Error msg
    elif System.Text.RegularExpressions.Regex.IsMatch(str,pattern) then
        Ok (ctor str)
    else
        let msg = $"%s{fieldName}: '%s{str}' must match the pattern '%s{pattern}'"
        Error msg 