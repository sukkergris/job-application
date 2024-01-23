module Main exposing (..)
import ContentText exposing (..)
import ColorTheme exposing (..)
import HeiselbergMsg exposing (..)
import HeiselbergButtons exposing (toggleThemeBtn)

import Element
import Element.Font
import Element.Region exposing (description)
import Element.Background
import Html exposing (Html)
import List exposing (head)
import Browser exposing (element)
import Element exposing (Element)
import Element exposing (paddingXY)


main =
    Browser.sandbox {
        init = themeLight
        , view = viewLayout
        , update = update
    }

update _ model =
    if model.theme == "Light" then
        themeDark
    else
        themeLight

fontNunitoRegular : Element.Attribute msg
fontNunitoRegular = Element.Font.family [Element.Font.typeface "NunitoRegular"]

fontWinterDrink : Element.Attribute msg
fontWinterDrink = Element.Font.family [Element.Font.typeface "WinterDrink"]


colors = {
    background = Element.rgb255 255 0 0
    , logoBackground = Element.rgb255 1 47 73
 }

topBanner : Element.Element msg
topBanner = Element.image [ Element.width Element.fill, Element.Background.color colors.logoBackground] {src = "/images/young-heiselberg.png", description = "Company logo"}

profileImage : Element.Element msg
profileImage = Element.image [Element.padding 22, Element.width (Element.maximum 150 Element.fill)] {src = "/cv/resources/profileimage-medium.jpg", description = "Image of Theodor Heiselberg"}


viewLayout : Theme -> Html Msg
viewLayout model=
    Element.layoutWith {
        options = []
    }
    []
    ( Element.column [Element.centerX, Element.Background.color model.primary ] 
    [
        topBanner
        , headlineParagrph model
        , toggleThemeBtn model
        , profileImage
        , contentParagaraph model
        , (buildingTheSiteParagraph rant model.textOnPrimary)
        , endParagraph
        , footer
    ])

textParagraph : Theme -> String -> Element.Element msg
textParagraph model txt = Element.paragraph [fontNunitoRegular, Element.paddingXY 22 22, Element.Font.color model.textOnPrimary] [Element.text txt]

textEndParagaph : String -> Element.Color -> Element msg
textEndParagaph txt fontColor = Element.paragraph [fontNunitoRegular, Element.Font.color fontColor, Element.paddingEach { top = 22, right = 22, bottom = 0, left = 22 }] [Element.text txt]
headlineParagrph : Theme -> Element.Element msg
headlineParagrph theme = textParagraph theme headlineTxt


contentParagaraph : Theme -> Element.Element msg
contentParagaraph theme = textParagraph theme aboutMeTxt


buildingTheSiteParagraph : String -> Element.Color -> Element msg
buildingTheSiteParagraph txt fontColor = Element.paragraph [fontNunitoRegular, Element.Font.color fontColor, paddingXY 22 22] [Element.text txt]

endParagraph : Element.Element msg
endParagraph = textEndParagaph "..." themeLight.textOnPrimary

technologiesParagraph : Theme -> Element.Element msg
technologiesParagraph theme = textParagraph theme techbnologiesTxt
footer : Element.Element msg
footer =  Element.paragraph [ fontWinterDrink,Element.Font.color themeLight.textOnPrimary, Element.paddingEach { top = 22, right = 22, bottom = 10, left = 22 }, Element.centerX ] [Element.text ".NET Udvikler - Theodor Heiselberg" ]