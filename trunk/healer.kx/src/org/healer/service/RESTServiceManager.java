package org.healer.service;

import java.lang.annotation.Annotation;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.concurrent.ConcurrentHashMap;


public class RESTServiceManager {

	private static RESTServiceManager manager = new RESTServiceManager();
	
	private static ConcurrentHashMap<String, RESTService> serviceMap = new ConcurrentHashMap<String, RESTService>();
	
	private static final String SERVICE_URI_PRIFIX = "/service/";
	
	private static final String SERVICE_PACKAGE_PRIFIX = "org.healer.services";
	
	static {
	}
	
	private RESTServiceManager() {
		
	}
	
	public static RESTServiceManager getInstance() {
		return manager;
	}
	
	public RESTService getService(String uri) {

		if (uri.startsWith(SERVICE_URI_PRIFIX))
		{
			String servicePath = uri.substring(SERVICE_URI_PRIFIX.length());

			
			String className = getServiceClassName(servicePath);
			
			try {
				RESTService service = serviceMap.get(className);
				if (service != null) {
					return service;
				}
				
				Class<?> clazz = (Class<?>) Thread.currentThread().getContextClassLoader().loadClass(className);
				Annotation annotation = clazz.getAnnotation(SingletonRESTService.class);
				if (annotation != null) {
					// Singleton
					Method method = clazz.getMethod("getInstance", new Class[]{ });
					if (method != null) {
						service = (RESTService) method.invoke(new Object[]{ });
						serviceMap.put(className, service);
					}
				} else {
					// more than one instance
					service = (RESTService) clazz.newInstance();
				}

				return service;
			} catch (ClassNotFoundException e) {
				e.printStackTrace();
			} catch (SecurityException e) {
				e.printStackTrace();
			} catch (NoSuchMethodException e) {
				e.printStackTrace();
			} catch (IllegalArgumentException e) {
				e.printStackTrace();
			} catch (IllegalAccessException e) {
				e.printStackTrace();
			} catch (InvocationTargetException e) {
				e.printStackTrace();
			} catch (InstantiationException e) {
				e.printStackTrace();
			}
		}
		
		return null;
	}
	
	private static String getServiceClassName(String servicePath) {
		if (servicePath.startsWith("files/file/")) {
			return "org.healer.services.files.FileService";
		}
		String[] parts = servicePath.split("/");
		String serviceName = parts[parts.length - 1];
		StringBuffer sb = new StringBuffer();
		sb.append(SERVICE_PACKAGE_PRIFIX).append(".");
		for (int i = 0; i < parts.length - 1; ++i) {
			sb.append(parts[i]).append(".");
		}
		
		sb.append(Character.toUpperCase(serviceName.charAt(0)));
		sb.append(serviceName.substring(1));
		sb.append("Service");
		
		return sb.toString();
	}
	
}
