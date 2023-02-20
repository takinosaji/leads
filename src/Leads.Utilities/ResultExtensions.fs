module Leads.Utilities.Result

open System
       
let private prepend firstR restR =
    match firstR, restR with
    | Ok first, Ok rest -> Ok(first :: rest)
    | Error err1, _ -> Error err1
    | Ok _, Error err2 -> Error err2
       
type Result<'a, 'b> with
    static member zip x1 x2 =
        match x1,x2 with
        | Ok x1res, Ok x2res -> Ok (x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e
        
    static member fromList aListOfResults =
        let initialValue = Ok[]
        List.foldBack prepend aListOfResults initialValue
        
    static member toList (resultOfaList: Result<'a list, 'b>) =
        match resultOfaList with
        | Ok list ->
            List.map (fun li -> Ok li) list
        | Error e -> [Error e]
    
let toOption result =
    match result with
    | Ok value -> Ok (Some value)
    | Error error -> Error error

type ResultBuilder() as self =
    member _.Return(x) = Ok x
    member _.ReturnFrom(m: 'T) = m
    member _.Bind(m, f) = Result.bind f m
    member _.Zero() = None
    member _.Combine(m, f) = Option.bind f m
    member _.Delay(f: unit -> _) = f
    member _.Run(f) = f()
    member _.TryWith(m, h) =
        try self.ReturnFrom(m)
        with e -> h e
    member _.TryFinally(m, compensation) =
        try self.ReturnFrom(m)
        finally compensation()
    member _.Using(res:#IDisposable, body) =
        self.TryFinally(body res, fun () ->
            match res with
            | null -> ()
            | disposable -> disposable.Dispose())
    member _.While(guard, f) =
        if not (guard()) then Some () else
        do f() |> ignore
        self.While(guard, f)
    member _.For(sequence:seq<_>, body) =
        self.Using(sequence.GetEnumerator(), fun enum -> self.While(enum.MoveNext, self.Delay(fun () -> body enum.Current)))        
    member _.MergeSources(t1: Result<'T,'U>, t2: Result<'T1,'U>) = Result.zip t1 t2
    member _.BindReturn(x: Result<'T,'U>, f) = Result.map f x
let result = ResultBuilder()    
