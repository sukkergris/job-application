module Main exposing (..)
import Element
import Element.Font
import Element.Region exposing (description)
import Element.Background
import Html exposing (Html)
import List exposing (head)
import Browser exposing (element)
import Element exposing (Element)
import Element exposing (paddingXY)

main : Html msg
main = viewLayout

fontNunitoRegular : Element.Attribute msg
fontNunitoRegular = Element.Font.family [Element.Font.typeface "NunitoRegular"]

fontWinterDrink : Element.Attribute msg
fontWinterDrink = Element.Font.family [Element.Font.typeface "WinterDrink"]

themeLight = {
    primary = Element.rgb255 0x1E 0x65 0x86
    , primaryLight = Element.rgb255 0xEA 0xDD 0xFFF
    , primaryDark = Element.rgb255 0xC5 0xe7 0xff
    , secondary = Element.rgb255 0x4E 0x61 0x6D
    , secondaryLight = Element.rgb255 0xf6 0xfa 0xfe
    , secondaryDark = Element.rgb255 0xd1 0xe5 0xf4
    , textOnPrimary = Element.rgb255 0xff 0xff 0xff
    , textOnSecondary = Element.rgb255 0xff 0xff 0xff
 }

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
    ( Element.column [Element.centerX, Element.Background.color themeLight.primary ] [
        topBanner
        , headlineParagrph
        , profileImage
        , contentParagaraph
        , (buildingTheSiteParagraph rant themeLight.textOnPrimary)
        , endParagraph
        , footer
    ])

textParagraph : String -> Element.Element msg
textParagraph txt = Element.paragraph [fontNunitoRegular, Element.paddingXY 22 22, Element.Font.color themeLight.textOnPrimary] [Element.text txt]

textEndParagaph txt fontColor = Element.paragraph [fontNunitoRegular, Element.Font.color fontColor, Element.paddingEach { top = 22, right = 22, bottom = 0, left = 22 }] [Element.text txt]
headlineParagrph : Element.Element msg
headlineParagrph = textParagraph headlineTxt

headlineTxt : String
headlineTxt = "Udvikler med speciale i Microsoft .NET og Azure"

contentParagaraph : Element.Element msg
contentParagaraph = textParagraph contentTxt

contentTxt : String
contentTxt = "Lidt om mig"

buildingTheSiteParagraph txt fontColor = Element.paragraph [fontNunitoRegular, Element.Font.color fontColor, paddingXY 22 22] [Element.text txt]

endParagraph : Element.Element msg
endParagraph = textEndParagaph "..." themeLight.textOnPrimary

technologiesParagraph : Element.Element msg
technologiesParagraph = textParagraph ".NET, EF Core, SQL, WPF, Blazor, Maui, GitHub Actions"
footer : Element.Element msg
footer =  Element.paragraph [ fontWinterDrink,Element.Font.color themeLight.textOnPrimary, Element.paddingEach { top = 22, right = 22, bottom = 10, left = 22 }, Element.centerX ] [Element.text ".NET Udvikler - Theodor Heiselberg" ]

rant : String
rant = "Så kom turen til design. Lige nu er det Material Design der leges med :)"