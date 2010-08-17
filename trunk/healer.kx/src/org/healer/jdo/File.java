package org.healer.jdo;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

public class File {


    private Long id;
	

	private String fileName;
	

	private byte[] content;

	public File(String fileName) {
		this.fileName = fileName;
		content = "hello world".getBytes();
	}

	public void setId(Long id) {
		this.id = id;
	}

	public Long getId() {
		return id;
	}

	public void setFileName(String fileName) {
		this.fileName = fileName;
	}


	public String getFileName() {
		return fileName;
	}

	public void setContent(byte[] content) {
		this.content = content;
	}


	public byte[] getContent() {
		return content;
	}

}
