package org.healer.gpass.beans;


public class Entry {
	
	public static final String NAME = "name";
	public static final String DESC = "desc";
	public static final String SECR = "secr";

	public long getId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public String getDescription() {
		return description;
	}
	
	public String getSecret() {
		return secret;
	}
	
	private long id = -1;

	private String name = null;
	
	private String description = null;
	
	private String secret = null;
	
	
	public Entry(RecordEntry re) {
		this.name = re.getName();
		this.description = re.getDescription();
		this.secret = re.getSecret();
		
		this.id = re.getId();
	}
	
}
