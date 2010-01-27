package org.healer.gpass.beans;

import javax.jdo.annotations.IdGeneratorStrategy;
import javax.jdo.annotations.IdentityType;
import javax.jdo.annotations.PersistenceCapable;
import javax.jdo.annotations.Persistent;
import javax.jdo.annotations.PrimaryKey;

import com.google.appengine.api.datastore.Key;

@PersistenceCapable(identityType = IdentityType.APPLICATION)
public class RecordEntry {

	@PrimaryKey
	@Persistent(valueStrategy = IdGeneratorStrategy.IDENTITY)
	private Key key;
	
	@Persistent
	private String name;

	@Persistent
	private String description;

	@Persistent
	private String secret;
	
	

	public RecordEntry() {
	}

	public RecordEntry(String name) {
		this.name = name;
	}

	public RecordEntry(String name, String description, String secret) {
		this.name = name;
		this.description = description;
		this.secret = secret;

	}

	public Key getKey() {
		return key;
	}

	public long getId() {
		return key.getId();
	}
	
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public void setKey(Key key) {
		this.key = key;
		
	}
	
	public String getSecret() {
		return secret;
	}
	
	public void setSecret(String secret) {
		this.secret = secret;
	}

}