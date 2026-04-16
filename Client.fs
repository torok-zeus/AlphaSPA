namespace AlphaSPA

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.UI.Html

[<JavaScript>]
module Client =

    [<SPAEntryPoint>]
    let Main () =
        
        let plateVar = Var.Create ""

        let spots =
            [1..10]
            |> List.map (fun i -> 
                let name = "A" + string i
                name, Var.Create<Option<string>>(None)
            )

        let renderSpot (name, spotVar: Var<Option<string>>) =
            let isOccupied =
                spotVar.View.Map Option.isSome

            let label =
                spotVar.View.Map (function
                    | Some plate -> name + "-" + plate
                )
            button [
                attr.styleDyn(
                    isOccupied.Map (fun occ ->
                        if occ then
                            "margin:5px; padding:15px; background-color:red; color:white; border none;"
                        else
                            "margin:5px; padding:15px; background-color:green; color:white; border none;"
                    )
                )
                on.click (fun _ _ ->
                    match spotVar.Value, plateVar.Value with
                    | None, plate when plate <> "" ->
                        spotVar.Value <- Some plate
                        plateVar.Value <- ""
                    | Some _, _ ->
                        ()
                    | _ -> ()
                )
                
            ] [
                textView label
            ]

        div [] [
            h3 [] [text "Parking System"]
            div [] [
               Doc.Input [attr.placeholder "Plate"] plateVar
            ]  

            div [] (spots |> List.map renderSpot)
        ]
        |> Doc.RunById "main"
        