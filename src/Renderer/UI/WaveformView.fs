(*
    WaveformView.fs

    A popup that allows to view simulation behaviour via generated waveforms.
*)

module WaveformView

open Fable.Helpers.React
open Fable.Helpers.React.Props

open PopupView

let private popupExtraStyle = [ Width "65%"; Height "80%" ]

/// Create a UI element representing the waveform, given a list of N values
/// representing how the signal evolved over time.
let private makeWaveform signalOverTime =
    // Stub.
    div [] []

/// Create waveforms for each output and state to show how they evolved during
/// the simulation.
let private makeBody outputs states model dispatch =
    List.map makeWaveform outputs @
    List.map makeWaveform states
    |> div []

/// Standard foot to close the popup should be enough.
let private makeFoot dispatch =
    // Stub.
    div [] []

/// Feed N clock ticks and return a list of N outputs and states of stateful
/// components, basically giving a snapshot of the system state after each
/// clock tick.
let private feedNClockTicks simData model dispatch =
    // Stub.
    [], []

/// Open a popup to visualise to simulation behaviour via generated waveforms.
let openWaveformViewer simData model dispatch : unit =
    let outputs, states = feedNClockTicks simData model dispatch
    let title = "Waveform viewer"
    let body = makeBody outputs states model dispatch
    let foot = makeFoot dispatch
    closablePopup title body foot popupExtraStyle dispatch
