package org.healer.service;

import java.io.IOException;
import javax.servlet.ServletConfig;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Servlet implementation class RESTServlet
 */
public class RESTServlet extends HttpServlet {
	
	private static final long serialVersionUID = 1L;
	
	private static RESTServiceManager serviceManager = RESTServiceManager.getInstance(); 
       
    /**
     * @see HttpServlet#HttpServlet()
     */
    public RESTServlet() {
        super();
    }

	/**
	 * @see Servlet#init(ServletConfig)
	 */
	public void init(ServletConfig config) throws ServletException {
	}

	/**
	 * @see Servlet#destroy()
	 */
	public void destroy() {
		serviceManager.destroy();
	}

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		
		String requestUri = request.getRequestURI();
		RESTService service = serviceManager.getService(requestUri);
		if (service != null)
		{
			service.onGet(request, response);
		}
	}

	/**
	 * @see HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		RESTService service = serviceManager.getService(request.getRequestURI());
		if (service != null)
		{
			service.onPost(request, response);
		}
	}

	/**
	 * @see HttpServlet#doPut(HttpServletRequest, HttpServletResponse)
	 */
	protected void doPut(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		RESTService service = serviceManager.getService(request.getRequestURI());
		if (service != null)
		{
			service.onPut(request, response);
		}
	}

	/**
	 * @see HttpServlet#doDelete(HttpServletRequest, HttpServletResponse)
	 */
	protected void doDelete(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		RESTService service = serviceManager.getService(request.getRequestURI());
		if (service != null)
		{
			service.onDelete(request, response);
		}
	}

}
