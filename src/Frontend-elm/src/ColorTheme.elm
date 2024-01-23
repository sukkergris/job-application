module ColorTheme exposing (themeLight,themeDark, Theme)
import Element
type alias Theme =
 { primary : Element.Color, primaryLight : Element.Color, primaryDark : Element.Color, secondary : Element.Color, secondaryLight : Element.Color, secondaryDark : Element.Color, textOnPrimary : Element.Color, textOnSecondary : Element.Color, theme : String }

themeLight : Theme
themeLight = {
    primary = Element.rgb255 0x1E 0x65 0x86
    , primaryLight = Element.rgb255 0xEA 0xDD 0xFFF
    , primaryDark = Element.rgb255 0xC5 0xe7 0xff
    , secondary = Element.rgb255 0x4E 0x61 0x6D
    , secondaryLight = Element.rgb255 0xf6 0xfa 0xfe
    , secondaryDark = Element.rgb255 0xd1 0xe5 0xf4
    , textOnPrimary = Element.rgb255 0xff 0xff 0xff
    , textOnSecondary = Element.rgb255 0xff 0xff 0xff
    , theme = "Light"
 }

themeDark : Theme
themeDark = {
    primary = Element.rgb255 0x27 0x41 0x53
    , primaryLight = Element.rgb255 0xBD 0x8D 0x9E
    , primaryDark = Element.rgb255 0x17 0x2A 0x36
    , secondary = Element.rgb255 0x70 0x80 0x8C
    , secondaryLight = Element.rgb255 0xED 0xEF 0xF5
    , secondaryDark = Element.rgb255 0x9E 0xB1 0xB9
    , textOnPrimary = Element.rgb255 0xD9 0xEC 0xF0
    , textOnSecondary = Element.rgb255 0xFF 0xFF 0xFF
    , theme = "Dark"
  }
