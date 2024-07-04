/// <summary>
/// Contains return values for Access Control Check.
/// </summary>
internal class AccessControlCheck
{
    /// <summary>
    /// Gets or Sets if the Access Control Check passed.
    /// </summary>
    /// <value>
    /// A <b>Boolean</b> indicating if the check passed.
    /// </value>
    internal bool IsPassed { get; set; } = false;
    /// <summary>
    /// Gets or Sets if a Challenge was processed.
    /// </summary>
    /// <value>
    /// A <b>Boolean</b> indicating if a Challenge was used.
    /// </value>
    internal bool IsChallenge { get; set; } = false;
    /// <summary>
    /// Gets or Sets the Challenge used.
    /// </summary>
    /// <value>
    /// A <b>String</b> containing the challenge used.
    /// </value>
    internal string Challenge { get; set; }
}