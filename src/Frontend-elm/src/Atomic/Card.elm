module Atomic.Card exposing (..)

import Element exposing (..)
import HeiselbergMsg exposing (Msg)


type alias CardContent =
    { headline : String, imageUrl : String, teaserText : String }


card : CardContent -> Element msg
card content =
    paragraph [ paddingXY 22 22 ]
        [ el [] (text content.headline)
        , image [ height (maximum 100 fill) ] { src = content.imageUrl, description = "Lille billede af Theodor A. H. Heiselberg " }
        ]
