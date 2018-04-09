﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DiscordPresence
{
	[Header("Basic Details")]

	/// <summary>
	/// The details about the game. Appears underneath the game name
	/// </summary>
	[Tooltip("The details about the game")]
	public string details = "Playing a game";

	/// <summary>
	/// The current state of the game (In Game, In Menu etc). Appears next to the party size
	/// </summary>
	[Tooltip("The current state of the game (In Game, In Menu). It appears next to the party size.")]
	public string state = "In Game";

	[Header("Time Details")]

	/// <summary>
	/// The time the game started. 0 if the game hasn't started
	/// </summary>
	[Tooltip("The time the game started. Leave as 0 if the game has not yet started.")]
	public DiscordTimestamp startTime = 0;

	/// <summary>
	/// The time the game will end in. 0 to ignore endtime.
	/// </summary>
	[Tooltip("Time the game will end. Leave as 0 to ignore it.")]
	public DiscordTimestamp endTime = 0;

	[Header("Presentation Details")]

	/// <summary>
	/// The images used for the presence.
	/// </summary>
	[Tooltip("The images used for the presence")]
	public DiscordAssets assets = new DiscordAssets();

	[Header("Party Details")]

	/// <summary>
	/// The current party
	/// </summary>
	[Tooltip("The current party. Identifier must not be empty")]
	public DiscordParty party = new DiscordParty("", 0, 0);

	/// <summary>
	/// The current secrets for the join / spectate feature.
	/// </summary>
	[Tooltip("The current secrets for the join / spectate feature.")]
	public DiscordSecrets secrets = new DiscordSecrets();

	/// <summary>
	/// Creates a new Presence object
	/// </summary>
	public DiscordPresence() { }

	/// <summary>
	/// Creats a new Presence object, copying values of the Rich Presence
	/// </summary>
	/// <param name="presence">The rich presence, often received by discord.</param>
	public DiscordPresence(DiscordRPC.RichPresence presence)
	{
		this.state = presence.State;
		this.details = presence.Details;

		this.party = presence.HasParty() ? new DiscordParty(presence.Party) : null;
		this.assets = presence.HasAssets() ? new DiscordAssets(presence.Assets) : null;
		this.secrets = presence.HasSecrets() ? new DiscordSecrets(presence.Secrets) : null;
		
		if (presence.HasTimestamps())
		{
			this.startTime = presence.Timestamps.Start.HasValue ? new DiscordTimestamp(presence.Timestamps.Start.Value) : new DiscordTimestamp(0);
			this.endTime = presence.Timestamps.End.HasValue ? new DiscordTimestamp(presence.Timestamps.End.Value) : new DiscordTimestamp(0);
		}
	}

	/// <summary>
	/// Converts this object into a new instance of a rich presence, ready to be sent to the discord client.
	/// </summary>
	/// <returns>A new instance of a rich presence, ready to be sent to the discord client.</returns>
	public DiscordRPC.RichPresence ToRichPresence()
	{
		var presence = new DiscordRPC.RichPresence();
		presence.State		= this.state;
		presence.Details	= this.details;

		presence.Assets		= this.assets.ToRichAssets();
		presence.Party		= this.party.ToRichParty();
		presence.Secrets	= this.secrets.ToRichSecrets();
		
		if (startTime > 0 || endTime > 0)
		{
			presence.Timestamps = new DiscordRPC.Timestamps();
			if (startTime > 0) presence.Timestamps.Start = startTime.GetDateTime();
			if (endTime > 0) presence.Timestamps.End = endTime.GetDateTime();
		}

		return presence;
	}

	public static explicit operator DiscordRPC.RichPresence(DiscordPresence presence)
	{
		return presence.ToRichPresence();
	}

	public static explicit operator DiscordPresence(DiscordRPC.RichPresence presence)
	{
		return new DiscordPresence(presence);
	}
}
