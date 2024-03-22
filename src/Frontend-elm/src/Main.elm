module Main exposing (..)

import About.About exposing (..)
import Atomic.Article exposing (Content, article)
import Atomic.Card exposing (..)
import Browser
import ColorTheme exposing (..)
import ContentText exposing (..)
import Element exposing (Element, Length, fill, paddingXY, paragraph, width)
import Element.Background
import Element.Font
import HeiselbergButtons exposing (toggleThemeBtn)
import HeiselbergMsg exposing (..)
import Html
import List exposing (maximum)


main =
    Browser.sandbox
        { init = themeLight
        , view = viewLayout
        , update = update
        }


update _ model =
    if model.theme == "Light" then
        themeDark

    else
        themeLight


fontNunitoRegular : Element.Attribute msg
fontNunitoRegular =
    Element.Font.family [ Element.Font.typeface "NunitoRegular" ]


fontWinterDrink : Element.Attribute msg
fontWinterDrink =
    Element.Font.family [ Element.Font.typeface "WinterDrink" ]


colors =
    { background = Element.rgb255 255 0 0
    , logoBackground = Element.rgb255 1 47 73
    }


topBanner : Element.Element msg
topBanner =
    Element.image [ Element.width Element.fill, Element.Background.color colors.logoBackground ] { src = "/images/young-heiselberg.png", description = "Company logo" }


profileImage : Element.Element msg
profileImage =
    Element.image
        [ Element.padding 22
        , Element.width (Element.maximum 150 Element.fill)
        ]
        { src = "/cv/resources/profileimage-medium.jpg"
        , description = "Billede af Theodor Heiselberg"
        }


viewLayout : Theme -> Html.Html Msg
viewLayout model =
    Element.layoutWith
        { options =
            []
        }
        []
        (Element.column
            [ Element.centerX
            , Element.Background.color model.background
            , Element.width (fill |> Element.maximum 1280)
            ]
            [ topBanner
            , paragraph [ paddingXY 22 22 ] [ toggleThemeBtn model ]
            , article model
                { headline = ".NET Udvikler - Siden 2008"
                , imageUrl = "/images/profileimage-article.jpg"
                , content = ContentText.developerArticle
                }
            , card { headline = ".NET Udvikler", imageUrl = "", teaserText = "Siden 2008" }
            , About.About.about
            , headlineParagrph model
            , profileImage
            , contentParagaraph model
            , buildingTheSiteParagraph rant model.textOnPrimary
            , endParagraph
            , footer
            ]
        )


textParagraph : Theme -> String -> Element.Element msg
textParagraph model txt =
    Element.paragraph [ fontNunitoRegular, Element.paddingXY 22 22, Element.Font.color model.textOnPrimary ] [ Element.text txt ]


textEndParagaph : String -> Element.Color -> Element msg
textEndParagaph txt fontColor =
    Element.paragraph [ fontNunitoRegular, Element.Font.color fontColor, Element.paddingEach { top = 22, right = 22, bottom = 0, left = 22 } ] [ Element.text txt ]


headlineParagrph : Theme -> Element.Element msg
headlineParagrph theme =
    textParagraph theme headlineTxt


contentParagaraph : Theme -> Element.Element msg
contentParagaraph theme =
    textParagraph theme aboutMeTxt


buildingTheSiteParagraph : String -> Element.Color -> Element msg
buildingTheSiteParagraph txt fontColor =
    Element.paragraph [ fontNunitoRegular, Element.Font.color fontColor, paddingXY 22 22 ] [ Element.text txt ]


endParagraph : Element.Element msg
endParagraph =
    textEndParagaph "..." themeLight.textOnPrimary


technologiesParagraph : Theme -> Element.Element msg
technologiesParagraph theme =
    textParagraph theme techbnologiesTxt


footer : Element.Element msg
footer =
    Element.paragraph [ fontWinterDrink, Element.Font.color themeLight.textOnPrimary, Element.paddingEach { top = 22, right = 22, bottom = 10, left = 22 }, Element.centerX ] [ Element.text ".NET Udvikler - Theodor Heiselberg" ]
