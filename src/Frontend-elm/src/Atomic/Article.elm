module Atomic.Article exposing (..)

import ColorTheme exposing (..)
import Element exposing (..)
import Element.Background
import Element.Border
import Element.Font
import Html exposing (br, math)
import Regex


type alias Content =
    { headline : String, imageUrl : String, content : String }


styledArticleParagraph : Theme -> List (Element msg) -> Element msg
styledArticleParagraph theme =
    column
        [ Element.Background.color theme.primary
        , Element.Font.color theme.textOnPrimary
        , paddingXY 0 22
        , Element.centerX
        , Element.Border.width 10
        , Element.Border.color theme.background
        ]


article : Theme -> Content -> Element msg
article theme c =
    styledArticleParagraph theme
        [ el [ paddingXY 22 0, Element.Font.size 30 ] (text c.headline)
        , image [ height fill, width fill, paddingXY 0 22 ]
            { src = c.imageUrl
            , description = "Stort billede af Theodor A. H. Heiselberg"
            }
        , paragraph [ paddingXY 22 0 ]
            [ el [] (text c.content)
            , Element.html <| br [] []
            , el [] (text c.content)
            ]
        ]


onNewline : Regex.Regex
onNewline =
    Maybe.withDefault Regex.never <| Regex.fromString "\\n"


test =
    Regex.find onNewline "jkl;asjkdl;jfklj\njklasjdlfkjasd\njkaskldfkl"
        |> List.map (\m -> Element.html <| br [] [])
