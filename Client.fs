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

        let menuOpen = Var.Create false

        let sidebarWidth = 200

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
        let hamburger =
            button[
                attr.style "position:fixed; top:10px; left:10px; z-index:1000; font-size:20px;"
                on.click (fun _ _ -> menuOpen.Value <- not menuOpen.Value)
            ] [
                text "///"
            ]

        let sidebar =
            div [
                attr.styleDyn (
                    menuOpen.View.Map (fun isOpen ->
                        let x =
                            if isOpen then "0px"
                            else "-" + string sidebarWidth + "px"

                        "position:fixed;
                          top:0;
                          left:{x}
                          width:{sidebarWidth}px
                          height:100%
                          background:#333
                          color:white;
                          padding:20px;
                          transition:left 0.3s ease;
                          z-index:1500;"
                    )
                )   
            ] [
                h4 [] [ text "Menu"]

                button [
                    attr.style "display:block; margin:10px 0;"
                    on.click (fun _ _ ->
                        currentPage.Value <- Parking
                        menuOpen.Value <- false
                    )
                ] [ text "Parking"]

                button [
                    attr.style "display:block; margin:10px 0;"
                    on.click (fun _ _ ->
                        currentPage.Value <- Payment
                        menuOpen.Value <- false
                    )
                ] [text "Payment"]
            ]

        let mainView =
            currentPage.View.Map (function
                | Parking -> parkingView
                | Payment -> paymentView
            )
        let mainContainer = 
            div [
                attr.styleDyn (
                     menuOpen.View.Map (fun isOpen ->
                        if isOpen then
                            $"margin-left:{sidebarWidth}px; transition:margin-left 0.3s ease;"
                        else
                            "margin-left:0px; transition:margin-left 0.3s ease;"
                    )
                )
            ] [
                Doc.BindView id mainView
            ]
        div [] [
            hamburger
            sidebar
            Doc.BindView id mainView
        ]
        |> Doc.RunById "main"