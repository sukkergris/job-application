module Atomic.Card exposing (..)

import Element exposing (..)
import Element.Border exposing (..)
import Element.Background
import HeiselbergMsg exposing (Msg)


type alias CardContent =
    { headline : String, imageUrl : String, teaserText : String }


card : CardContent -> Element msg
card content =
    paragraph [
        paddingXY 22 22
        , rounded 7
        , Element.Background.color <| Element.rgb255 0x4E 0x61 0x6D]
        [ el [] (text content.headline)
        , image [ height (maximum 100 fill) ] { src = content.imageUrl, description = "Lille billede af Theodor A. H. Heiselberg ss " }
        ]
