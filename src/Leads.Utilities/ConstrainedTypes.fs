module Leads.Utilities.ConstrainedTypes

open System

type ErrorText = ErrorText of string
let errorTextToString errorText =
    let (ErrorText text) = errorText
    text
let stringToErrorText errorString = ErrorText errorString

let createNotEmptyString fieldName ctor str = 
    if String.IsNullOrEmpty(str) then
        let msg = $"%s{fieldName} must not be null or empty" 
        Error(ErrorText msg)
    else
        Ok (ctor str)            

let createLimitedString fieldName ctor maxLen str = 
    if String.IsNullOrEmpty(str) then
        let msg = $"%s{fieldName} must not be null or empty" 
        Error(ErrorText msg)
    elif str.Length > maxLen then
        let msg = $"%s{nameof(ctor)} must not be more than %i{maxLen} chars" 
        Error(ErrorText msg)
    else
        Ok (ctor str)

let createPredefinedString fieldName ctor str (allowedValues:string list) = 
    if not (List.contains str allowedValues) then
        let msg = $"%s{fieldName}'s value must be in range of allowed values" 
        Error(ErrorText msg)
    else
        Ok (ctor str)

let createStringOption fieldName ctor maxLen str = 
    if String.IsNullOrEmpty(str) then
        Ok None
    elif str.Length > maxLen then
        let msg = $"%s{fieldName} must not be more than %i{maxLen} chars" 
        Error(ErrorText msg) 
    else
        Ok (ctor str |> Some)


let createInt fieldName ctor minVal maxVal i = 
    if i < minVal then
        let msg = $"%s{fieldName}: Must not be less than %i{minVal}"
        Error(ErrorText msg)
    elif i > maxVal then
        let msg = $"%s{fieldName}: Must not be greater than %i{maxVal}"
        Error(ErrorText msg)
    else
        Ok (ctor i)

let createFloat fieldName ctor minVal maxVal i = 
    if i < minVal then
        let msg = $"%s{fieldName}: Must not be less than %f{minVal}"
        Error(ErrorText msg)
    elif i > maxVal then
        let msg = $"%s{fieldName}: Must not be greater than %f{maxVal}"
        Error(ErrorText msg)
    else
        Ok (ctor i)

let createDecimal fieldName ctor minVal maxVal i = 
    if i < minVal then
        let msg = $"%s{fieldName}: Must not be less than %M{minVal}"
        Error(ErrorText msg)
    elif i > maxVal then
        let msg = $"%s{fieldName}: Must not be greater than %M{maxVal}"
        Error(ErrorText msg)
    else
        Ok (ctor i)

let createLike fieldName ctor pattern str = 
    if String.IsNullOrEmpty(str) then
        let msg = $"%s{fieldName}: Must not be null or empty" 
        Error(ErrorText msg)
    elif System.Text.RegularExpressions.Regex.IsMatch(str,pattern) then
        Ok (ctor str)
    else
        let msg = $"%s{fieldName}: '%s{str}' must match the pattern '%s{pattern}'"
        Error(ErrorText msg)