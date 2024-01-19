module Main exposing (..)
import Element
import Element.Font
import Element.Region exposing (description)
import Element.Background
import Html exposing (Html)
import List exposing (head)
import Browser exposing (element)

main : Html msg
main = viewLayout

fontNunitoRegular : Element.Attribute msg
fontNunitoRegular = Element.Font.family [Element.Font.typeface "NunitoRegular"]

fontWinterDrink : Element.Attribute msg
fontWinterDrink = Element.Font.family [Element.Font.typeface "WinterDrink"]
colors : { background : Element.Color, logoBackground : Element.Color }
colors = {
    background = Element.rgb255 255 0 0
    , logoBackground = Element.rgb255 1 47 73
 }
red : Element.Color
red = Element.rgb255 255 0 0

topBanner : Element.Element msg
topBanner = Element.image [ Element.width Element.fill, Element.Background.color colors.logoBackground] {src = "/images/young-heiselberg.png", description = "Company logo"}

profileImage : Element.Element msg
profileImage = Element.image [Element.padding 22, Element.width (Element.maximum 150 Element.fill)] {src = "/cv/resources/profileimage-medium.jpg", description = "Image of Theodor Heiselberg"}

viewLayout : Html msg
viewLayout =
    Element.layoutWith {
        options = []
    }
    []
    ( Element.column [Element.centerX] [
        topBanner
        , headlineParagrph
        , profileImage
        , contentParagaraph
        , endParagraph
        , footer
    ])

textParagraph : String -> Element.Element msg
textParagraph txt = Element.paragraph [fontNunitoRegular, Element.paddingXY 22 22] [Element.text txt]

textEndParagaph : String -> Element.Element msg
textEndParagaph txt = Element.paragraph [fontNunitoRegular, Element.paddingEach { top=22, right=22, bottom=0, left=22 }] [Element.text txt]
headlineParagrph : Element.Element msg
headlineParagrph = textParagraph headlineTxt

headlineTxt : String
headlineTxt = "Udvikler med speciale i Microsoft .NET og Azure"

contentParagaraph : Element.Element msg
contentParagaraph = textParagraph contentTxt

contentTxt : String
contentTxt = "Lidt om mig"

endParagraph : Element.Element msg
endParagraph = textEndParagaph "..."

technologiesParagraph : Element.Element msg
technologiesParagraph = textParagraph ".NET, EF Core, SQL, WPF, Blazor, Maui, GitHub Actions"
footer : Element.Element msg
footer =  Element.paragraph [ fontWinterDrink, Element.paddingEach { top = 22, right = 22, bottom = 0, left = 22 }, Element.centerX ] [Element.text ".NET Udvikler - Theodor Heiselberg" ]
