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
    /// Gets or Sets a Challenge for further validation.
    /// </summary>
    /// <value>
    /// A <b>String</b> containing challenge response expected.
    /// </value>
    public string Challenge { get; set; }
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