package org.healer.jdo;

import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Inheritance;
import javax.persistence.InheritanceType;
import javax.persistence.Table;


public abstract class Item {

	/**
	 * 
	 */
	private Long id;

	public void setId(Long id) {
		this.id = id;
	}


	public Long getId() {
		return id;
	}
	
	
	
	
	
}
