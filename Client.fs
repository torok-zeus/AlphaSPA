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

        let mainView =
            currentPage.View.Map (function
                | Parking -> parkingView
                | Payment -> paymentView
            )
        let topBar =
            div [
                attr.style "
                    position:fixed;
                    top:0;
                    left:0;
                    width:100%;
                    height:50px;
                    background:#222;
                    color:white;
                    display:flex;
                    align-items:center;
                    padding:0 10px;
                    z-index:3000;
                "
            ] [
                button [
                    attr.style "font-size:20px; background:none; border:none; color:white; cursor:pointer;"
                    on.click (fun _ _ -> menuOpen.Value <- not menuOpen.Value)
                ] [ text "///"]
                div [attr.style "margin:10px"] [
                    text "Parking App"
                ]
            ]
        let layout =
            div [
                attr.style "display:flex; margin-top:50px; height:calc(100vh - 50px);"
            ] [
                div [
                    attr.styleDyn (
                        menuOpen.View.Map (fun isOpen ->
                            if isOpen then
                                 "width:200px; background:#333; color:white; padding:10px; transition:0.3s;"
                            else
                                 "width:0px; overflow:hidden; transition:0.3s;"                               
                        )
                    )
                ] [
                    button [
                        attr.style "display:block; margin:10px 0;"
                        on.click (fun _ _ ->
                            currentPage.Value <- Parking
                        )
                    ] [ text "Parking"]
                    
                    button [
                        attr.style "display:block; margin:10px 0;"
                        on.click (fun _ _ ->
                            currentPage.Value <- Payment
                        )
                    ] [ text "Payment"]
                 
                ]
                div [
                     attr.style "flex:1; padding:10px;"
                ] [
                     Doc.BindView id mainView
                ]
            ]

        
        div [] [
            topBar
            layout
            Doc.BindView id mainView
        ]
        |> Doc.RunById "main"