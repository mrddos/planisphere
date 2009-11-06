package org.healer.gpass.beans;

import java.util.Date;

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
    private Key name;

	@Persistent
    private String description;

	@Persistent
    private String secret;


    public RecordEntry(String description) {
    	this.description = description;
    }
    
    public RecordEntry(String description, String secret) {
    	this.description = description;
    	this.secret = secret; 
    		
	}

	public Key getKey() {
    	return name;
    }
	
	
	public String getDescription() {
		return description;
	}
	
	public void setDescription(String description) {
		this.description = description;
	}



    
}