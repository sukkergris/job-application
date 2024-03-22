module ColorTheme exposing (Theme, themeDark, themeLight)

import Element


type alias Theme =
    { primary : Element.Color, primaryLight : Element.Color, primaryDark : Element.Color, secondary : Element.Color, secondaryLight : Element.Color, secondaryDark : Element.Color, textOnPrimary : Element.Color, textOnSecondary : Element.Color, theme : String, background : Element.Color }


themeLight : Theme
themeLight =
    { primary = Element.rgb255 0x1E 0x65 0x86
    , primaryLight = Element.rgb255 0xEA 0xDD 0x0FFF
    , primaryDark = Element.rgb255 0xC5 0xE7 0xFF
    , secondary = Element.rgb255 0x4E 0x61 0x6D
    , secondaryLight = Element.rgb255 0xF6 0xFA 0xFE
    , secondaryDark = Element.rgb255 0xD1 0xE5 0xF4
    , textOnPrimary = Element.rgb255 0xFF 0xFF 0xFF
    , textOnSecondary = Element.rgb255 0xFF 0xFF 0xFF
    , theme = "Light"
    , background = Element.rgb255 0xF6 0xFA 0xFE
    }


themeDark : Theme
themeDark =
    { primary = Element.rgb255 0x27 0x41 0x53
    , primaryLight = Element.rgb255 0xBD 0x8D 0x9E
    , primaryDark = Element.rgb255 0x17 0x2A 0x36
    , secondary = Element.rgb255 0x70 0x80 0x8C
    , secondaryLight = Element.rgb255 0xED 0xEF 0xF5
    , secondaryDark = Element.rgb255 0x9E 0xB1 0xB9
    , textOnPrimary = Element.rgb255 0xD9 0xEC 0xF0
    , textOnSecondary = Element.rgb255 0xFF 0xFF 0xFF
    , theme = "Dark"
    , background = Element.rgb255 0x0F 0x14 0x17
    }
