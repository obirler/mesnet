namespace Mesnet.Classes.Ui
{
    /// <summary>
    /// Defines the current state of the mouse handling logic.
    /// </summary>
    public enum MouseHandlingMode
    {
        /// <summary>
        /// Not in any special mode.
        /// </summary>
        None,

        /// <summary>
        /// The user is left-dragging rectangles with the mouse.
        /// </summary>
        Dragging,

        /// <summary>
        /// The user is left-mouse-button-dragging to pan the viewport.
        /// </summary>
        Panning,

        /// <summary>
        /// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
        /// </summary>
        Zooming,

        /// <summary>
        /// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
        /// </summary>
        DragZooming,

        /// <summary>
        /// The user selects the circle start or top of the beam and selects a support.
        /// </summary>
        Assembling,

        /// <summary>
        /// The user tap on the canvas where new beam will be placed.
        /// </summary>
        BeamPlacing,

        /// <summary>
        /// The user has clicked two beam tip point to put a beam in order to form circular-boung beam system.
        /// </summary>
        CircularBeamConnection
    }
}
