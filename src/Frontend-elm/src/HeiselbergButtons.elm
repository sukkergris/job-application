module HeiselbergButtons exposing (toggleThemeBtn)
import ColorTheme exposing (..)
import Element.Input
import Element.Background
import Element
import HeiselbergMsg exposing (..)
import Element.Border
import Element.Font


toggleThemeBtn : Theme -> Element.Element Msg
toggleThemeBtn theme =
    Element.Input.button 
    [
        Element.Background.color theme.primaryDark
        , Element.Border.rounded 8
        , Element.Font.color theme.textOnSecondary
        , Element.alignRight
        , Element.paddingEach {top = 12, right = 12, bottom = 9, left = 12}
        , Element.Font.bold
        , Element.mouseOver 
            [Element.Background.color theme.primary]
    ]
    {
        onPress = Just MsgToggleTheme
        , label = Element.text "Toggle theme"
    }