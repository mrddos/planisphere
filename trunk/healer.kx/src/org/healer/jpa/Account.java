package org.healer.jpa;

import javax.jdo.annotations.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "Account")
public class Account {

	private Long id;
	
	private String userEmail;
	
	private String description;
	
	private double count;
	
	private String type;
	
	
	protected Account() {

	}
	
	public Account(String userEmail) {
		this.userEmail = userEmail;
	}
	


	@Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
	public Long getId() {
		return id;
	}

	public void setId(Long id) {
		this.id = id;
	}



	@Column
	public String getUserEmail() {
		return userEmail;
	}

	public void setUserEmail(String userEmail) {
		this.userEmail = userEmail;
	}

	
	public String toString() {
		return String.format("# %s ID: %d", userEmail, id.longValue());
	}

	@Column
	public String getDescription() {
		return description;
	}
	
	public void setDescription(String description) {
		this.description = description;
	}

	@Column
	public double getCount() {
		return count;
	}
	
	public void setCount(double count) {
		this.count = count;
	}

	@Column
	public String getType() {
		return type;
	}
	
	public void setType(String type) {
		this.type = type;
	}
}
