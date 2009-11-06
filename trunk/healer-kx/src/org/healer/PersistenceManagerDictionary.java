package org.healer;


import java.util.HashMap;

import javax.jdo.JDOHelper;
import javax.jdo.PersistenceManager;
import javax.jdo.PersistenceManagerFactory;





public class PersistenceManagerDictionary {
	

	private static HashMap<String, PersistenceManagerFactory> factoryDict = new HashMap<String, PersistenceManagerFactory>();
	
    
        
    private PersistenceManagerDictionary() {}

    public static PersistenceManagerFactory getFactory(String factoryName) {
        
    	if (factoryDict.containsKey(factoryName)) {
    		return factoryDict.get(factoryName);
    	}
    	else {
    		PersistenceManagerFactory factory = JDOHelper.getPersistenceManagerFactory(factoryName);
    		factoryDict.put(factoryName, factory);
    		return factory;
    	}

    }
    
    public static PersistenceManager get(String factoryName) {
        
    	if (factoryDict.containsKey(factoryName)) {
    		return factoryDict.get(factoryName).getPersistenceManager();
    	}
    	else {
    		PersistenceManagerFactory factory = JDOHelper.getPersistenceManagerFactory(factoryName);
    		factoryDict.put(factoryName, factory);
    		return factory.getPersistenceManager();
    	}

    }

}
