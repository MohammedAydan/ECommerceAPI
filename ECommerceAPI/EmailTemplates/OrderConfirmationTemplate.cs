namespace ECommerceAPI.EmailTemplates  
{  
   public class OrderConfirmationTemplate  
   {  
       public string Subject { get; set; }  
       public string Body { get; set; }  
        

       public OrderConfirmationTemplate(string userName, string orderId, string orderDate, string shippingAddress, string totalAmount, string paymentMethod, string orderUrl)  
       {  
           Subject = $"Order Confirmation - {orderId}";
           Body = $@"  
               <!DOCTYPE html>  
               <html lang=""en"">  
               <head>  
                 <meta charset=""UTF-8"" />  
                 <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>  
                 <title>Order Confirmation</title>  
                 <style>  
                   body {{  
                     margin: 0;  
                     background-color: #f9f9f9;  
                     font-family: 'Segoe UI', 'Roboto', sans-serif;  
                     color: #333;  
                   }}  
                   .email-container {{  
                     max-width: 600px;  
                     margin: 40px auto;  
                     background: #fff;  
                     border-radius: 8px;  
                     overflow: hidden;  
                     box-shadow: 0 4px 10px rgba(0, 0, 0, 0.05);  
                   }}  
                   .email-header {{  
                     background-color: black;  
                     color: #fff;  
                     padding: 24px;  
                     text-align: center;  
                   }}  
                   .email-body {{  
                     padding: 32px 24px;  
                   }}  
                   .email-body h2 {{  
                     margin-top: 0;  
                     color: #111;  
                   }}  
                   .order-summary {{  
                     background-color: #f0f0f0;  
                     padding: 16px;  
                     border-radius: 6px;  
                     margin: 20px 0;  
                   }}  
                   .order-summary p {{  
                     margin: 6px 0;  
                   }}  
                   .cta-button {{  
                     display: inline-block;  
                     margin-top: 24px;  
                     background-color: black;  
                     color: #fff;  
                     text-decoration: none;  
                     padding: 12px 24px;  
                     border-radius: 6px;  
                     font-weight: 600;  
                   }}  
                   .email-footer {{  
                     font-size: 13px;  
                     text-align: center;  
                     padding: 20px;  
                     color: #aaa;  
                     background-color: #fafafa;  
                   }}  
                 </style>  
               </head>  
               <body>  
                 <div class=""email-container"">  
                   <div class=""email-header"">  
                     <h1>Thank You for Your Order!</h1>  
                   </div>  
                   <div class=""email-body"">  
                     <h2>Hello, {userName}</h2>  
                     <p>Your order has been successfully placed. Below are your order details:</p>  

                     <div class=""order-summary"">  
                       <p><strong>Order ID:</strong> {orderId}</p>  
                       <p><strong>Order Date:</strong> {orderDate}</p>  
                       <p><strong>Shipping Address:</strong> {shippingAddress}</p>  
                       <p><strong>Total Amount:</strong> ${totalAmount}</p>  
                       <p><strong>Payment Method:</strong> {paymentMethod}</p>  
                     </div>  

                     <p>You can view your order history or track your shipment from your dashboard.</p>  

                     <a href=""{orderUrl}"" class=""cta-button"">View Order</a>  
                   </div>  
                   <div class=""email-footer"">  
                     &copy; {DateTime.Now.Year} MAG E-Commerce. All rights reserved.  
                   </div>  
                 </div>  
               </body>  
               </html>";  
       }  
   }  
}