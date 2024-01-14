module Main exposing (..)
import Element
import Element.Font
import Element.Region exposing (description)
import Element.Background

main = viewLayout

red = Element.rgb255 255 0 0

viewLayout =
    Element.layoutWith {
        options = []
    }
    [Element.Font.bold, Element.Font.color red]
    ( Element.column [] [
        Element.image [ Element.Background.color ( Element.rgb255 1 47 73)] {src = "/images/young-heiselberg.png", description = "logo"}
        , Element.text "Let's get this party started!"
        , viewParagraph
    ])

viewParagraph = Element.paragraph []
    [Element.text "Young Heiselberg - is wicked!"]
