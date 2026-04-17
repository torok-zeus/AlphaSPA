namespace AlphaSPA

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.UI.Html

[<JavaScript>]
module Client =
    type Page = 
        | Parking
        | Payment

    type Spot = {
        Plate: string
        StartTime: System.DateTime
    }

    [<SPAEntryPoint>]
    let Main () =
        
        let currentPage = Var.Create Parking
        let plateVar = Var.Create ""

        let spots =
            [1..10]
            |> List.map (fun i -> 
                let name = "A" + string i
                name, Var.Create<Option<Spot>>(None)
            )

        let renderSpot (name, spotVar: Var<Option<Spot>>) =
            let isOccupied =
                spotVar.View.Map Option.isSome

            let label =
                spotVar.View.Map (function
                    | Some s -> name + "-" + s.Plate
                    | None -> name
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
                        spotVar.Value <- Some{
                            Plate = plate
                            StartTime = System.DateTime.Now
                        }
                        plateVar.Value <- ""
                    | _ -> ()
                )
                
            ] [
                textView label
            ]
        let clacPrice (start: System.DateTime) =
            let minutes = (System.DateTime.Now - start).TotalMinutes
            int minutes * 100

        let paymentView =
            div [] [
                h3 [] [text "Payment"]
                div [] (
                    spots
                    |> List.map (fun (name, spotVar) ->
                        div [] [
                            textView (
                                spotVar.View.Map (function
                                    | Some s ->
                                        let price = clacPrice s.StartTime
                                        name + "-" + s.Plate + "-" + string price + " FT"
                                    | None ->
                                        name + " - empty"
                                )
                            )
                        ]
                    )
                )
            ]
        let parkingView =
            div [] [
                h3 [] [text "Parking System"]

                div [] [
                    Doc.InputType.Text [attr.placeholder "Plate"] plateVar
                ]

                div [] (spots |> List.map renderSpot)
            ]

        let menu =
            div [attr.style "margin-bottom:20px;"] [
                button [on.click (fun _ _ -> currentPage.Value <- Parking)] [text "Parking"]
                button [on.click (fun _ _ -> currentPage.Value <- Payment)] [text "Payment"]
            ]
        let mainView =
            currentPage.View.Map (function
                | Parking -> parkingView
                | Payment -> paymentView
            )
        div [] [
            menu
            Doc.BindView id mainView
        ]
        |> Doc.RunById "main"