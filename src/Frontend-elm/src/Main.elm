module Main exposing (..)
import Element
import Element.Font
import Element.Region exposing (description)
import Element.Background
import Html exposing (Html)

main : Html msg
main = viewLayout

red : Element.Color
red = Element.rgb255 255 0 0

viewLayout : Html msg
viewLayout =
    Element.layoutWith {
        options = []
    }
    [Element.Font.bold, Element.Font.color red]
    ( Element.column [] [
        Element.image [ Element.Background.color ( Element.rgb255 1 47 73)] {src = "/images/young-heiselberg.png", description = "Company logo"}
        , Element.text "Let's get this party started!"
        , viewParagraph
    ])

viewParagraph : Element.Element msg
viewParagraph = Element.paragraph []
    [Element.text "Elm lang - is wicked!"]
