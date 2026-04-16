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
        let spots =
            [1..10]
            |> List.map (fun i -> "A" + string i)

        let renderSpot spot =
            button [
                attr.style "margin:5px; padding:15px; background-color:green; color:white; border none;"
            ] [
                text spot
            ]

        div [] [
            h3 [] [text "Parking System"]
            div [] (spots |> List.map renderSpot)
        ]
        |> Doc.RunById "main"
