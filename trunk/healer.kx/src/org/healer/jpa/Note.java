package org.healer.jpa;

import javax.persistence.Column;



public class Note {
	
	private String name;
	
	private String content;
	
	private String type;

	public void setType(String type) {
		this.type = type;
	}

	@Column
	public String getType() {
		return type;
	}

	public void setContent(String content) {
		this.content = content;
	}

	@Column(length = 512)
	public String getContent() {
		return content;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getName() {
		return name;
	}

}
