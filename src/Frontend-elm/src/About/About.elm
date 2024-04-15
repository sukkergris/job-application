module About.About exposing (..)

import Element exposing (..)


about : Element msg
about =
    el [ paddingXY 22 22 ] (text "hello there")
