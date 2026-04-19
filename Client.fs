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

        let selectedSpot = Var.Create<Option<string>>(None)

        let now = Var.Create(System.DateTime.Now)

        let tick () =
            now.Value <- System.DateTime.Now

        JS.SetInterval tick, 1000 |> ignore

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
                    selectedSpot.Value <- Some name
                )
                
            ] [
                textView label
            ]
        let clacPrice (start: System.DateTime) =
            let seconds = (System.DateTime.Now - start).TotalSeconds
            int seconds

        let paymentView =
            div [] [
                h3 [] [text "Payment"]
                div [] (
                    spots
                    |> List.map (fun (name, spotVar) ->
                        div [] [
                            textView (
                                View.Map2 (fun spot nowTime ->
                                    match spot with
                                    | Some s ->
                                        let price = int ((nowTime - s.StartTime).TotalSeconds)
                                        name + "-" + s.Plate + "-" + string price + " FT"
                                    | None ->
                                        name + " - empty"                      
                                ) spotVar.View now.View
                               
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

                div [] [
                    textView (
                        selectedSpot.View.Map (function
                            | Some s -> "Parking space: " + s
                            | None -> " Parking space: none selected"
                        )
                    )
                ]

                button [
                    attr.style "margin-top:10px; padding:10px;"
                    on.click (fun _ _ ->
                        match selectedSpot.Value, plateVar.Value with
                        | Some name, plate when plate <> "" ->

                            let spotVar =
                                spots
                                |> List.find (fun (n, _) -> n = name)
                                |> snd

                            match spotVar.Value with
                            | None ->
                                spotVar.Value <- Some {
                                    Plate = plate
                                    StartTime = System.DateTime.Now
                                }
                                plateVar.Value <- ""
                                selectedSpot.Value <- None
                            | _ -> ()
                        | _ -> ()
                    )
                ] [ text "Park"]
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
                ] [ 
                    textView (
                        menuOpen.View.Map ( fun isOpen ->
                            if isOpen then "///" else "|||"
                        )
                    )
                ]
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
        ]
        |> Doc.RunById "main"