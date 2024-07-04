using System.Collections.Generic;
using System.Net;

/// <summary>
/// Contains a rule for Access Control.
/// </summary>
public class AccessControlRule
{
    /// <summary>
    /// Private field for the IP Address.
    /// </summary>
    private IPAddress ipaddress;
    /// <summary>
    /// Gets the IP Address.
    /// </summary>
    /// <value>
    /// An <b>IPAddress</b> containing an IP Address.
    /// </value>
    public IPAddress IPAddress => ipaddress;
    /// <summary>
    /// Challenges for further validation which can also be individually enabled or disabled.
    /// </summary>
    /// <value>
    /// A <b>Dictionary<string, bool></b> containing challenges expected and if the challenge is enabled or disabled.
    /// </value>
    public Dictionary<string, bool> Challenges { get; private set; } = new Dictionary<string, bool>();
    /// <summary>
    /// Gets or Sets the flag for enabling or disabling this rule.
    /// </summary>
    /// <value>
    /// A <b>Boolean</b> indicating if this rule is active or not.
    /// </value>
    public bool IsEnabled { get; set; } = true;
    /// <summary>
    /// Gets or Sets the flag for enabling or disabling the Challenge validation for this rule.
    /// </summary>
    /// <value>
    /// A <b>Boolean</b> indicating if Challenge for this rule is active or not.
    /// </value>
    public bool IsChallengeEnabled { get; set; } = false;

    /// <summary>
    /// Creates a new instance of the <see cref="AccessControlRule" /> with the specified IP Address.
    /// </summary>
    /// <param name="IPAddress">
    /// IP Address for the rule.
    /// </param>
    public AccessControlRule(IPAddress IPAddress)
    {
        ipaddress = IPAddress;
    }
}