package org.healer.jpa;

import javax.jdo.annotations.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "Tag")
public class Tag {
	
	private Long id;
	
	private String itemId;
	
	private String itemType;
	
	
	private String name;
	
	protected Tag() {
		
	}
	
	public Tag(String itemId, String itemType, String name) {
		this.itemId = itemId;
		this.itemType = itemType;
		this.name = name;
	}


	public void setId(Long id) {
		this.id = id;
	}

	@Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
	public Long getId() {
		return id;
	}


	public void setName(String name) {
		this.name = name;
	}

	@Column(length = 32)
	public String getName() {
		return name;
	}

	public void setItemId(String itemId) {
		this.itemId = itemId;
	}

	@Column(length = 64)
	public String getItemId() {
		return itemId;
	}

	public void setItemType(String itemType) {
		this.itemType = itemType;
	}

	@Column(length = 64)
	public String getItemType() {
		return itemType;
	}
}
