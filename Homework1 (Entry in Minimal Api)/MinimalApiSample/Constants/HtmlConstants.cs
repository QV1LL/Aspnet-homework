namespace MinimalApiSample.Constants;

public static class HtmlConstants
{
    public const string ProductsMarkup = """

                                         <!DOCTYPE html>
                                         <html lang="en">
                                         <head>
                                             <meta charset="UTF-8">
                                             <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                             <title>Simple Product Gallery</title>
                                             <style>
                                                 body { font-family: sans-serif; background-color: #f4f4f4; padding: 20px; }
                                                 .product-grid {
                                                     display: grid;
                                                     grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
                                                     gap: 20px;
                                                     max-width: 1200px;
                                                     margin: 0 auto;
                                                 }
                                                 .product-card {
                                                     background: white;
                                                     padding: 15px;
                                                     border-radius: 8px;
                                                     box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                                                     text-align: center;
                                                 }
                                                 .product-card img { max-width: 100%; border-radius: 4px; }
                                                 .price { color: #2ecc71; font-weight: bold; font-size: 1.2em; }
                                                 button {
                                                     background: #3498db;
                                                     color: white;
                                                     border: none;
                                                     padding: 10px 15px;
                                                     border-radius: 4px;
                                                     cursor: pointer;
                                                     margin-top: 10px;
                                                 }
                                                 button:hover { background: #2980b9; }
                                             </style>
                                         </head>
                                         <body>

                                             <h1 style="text-align:center;">Our Products</h1>

                                             <div class="product-grid">
                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>Wireless Mouse</h3>
                                                     <p class="price">$25.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>Mechanical Keyboard</h3>
                                                     <p class="price">$85.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>Gaming Headset</h3>
                                                     <p class="price">$50.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>Webcam HD</h3>
                                                     <p class="price">$45.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>USB-C Hub</h3>
                                                     <p class="price">$30.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>Monitor Stand</h3>
                                                     <p class="price">$20.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>LED Desk Lamp</h3>
                                                     <p class="price">$15.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>Laptop Sleeve</h3>
                                                     <p class="price">$18.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>External SSD</h3>
                                                     <p class="price">$120.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>

                                                 <div class="product-card">
                                                     <img src="https://via.placeholder.com/150" alt="Product">
                                                     <h3>Bluetooth Speaker</h3>
                                                     <p class="price">$40.00</p>
                                                     <button>Add to Cart</button>
                                                 </div>
                                             </div>

                                         </body>
                                         </html>

                                             
                                         """;
    
    public const string MirrorHeadersMarkup = """

                                         <!DOCTYPE html>
                                         <html>
                                         <head>
                                             <title>Header Mirror</title>
                                             <style>
                                                 body { font-family: sans-serif; background: #1a1a1a; color: #fff; padding: 2rem; }
                                                 .container { max-width: 800px; margin: 0 auto; }
                                                 .header-card { 
                                                     background: #2d2d2d; 
                                                     padding: 1rem; 
                                                     margin-bottom: 0.5rem; 
                                                     border-left: 4px solid #61dafb; 
                                                     display: flex;
                                                     justify-content: space-between;
                                                 }
                                                 .key { color: #61dafb; font-weight: bold; }
                                                 .value { color: #e5e5e5; word-break: break-all; margin-left: 20px; }
                                             </style>
                                         </head>
                                         <body>
                                             <div class='container'>
                                                 <h1>Request Headers</h1>
                                                 {{CONTENT}}
                                             </div>
                                         </body>
                                         </html>
                                 """;
    
    public const string ProfilePageMarkup = """

                                                <!DOCTYPE html>
                                                <html>
                                                <head>
                                                    <title>User Profile</title>
                                                    <style>
                                                        :root { --user-color: {{FAVORITE_COLOR}}; }
                                                        body { font-family: 'Segoe UI', sans-serif; background: #f0f2f5; margin: 0; display: flex; justify-content: center; align-items: center; height: 100vh; }
                                                        .profile-card { background: white; width: 500px; border-radius: 15px; overflow: hidden; box-shadow: 0 10px 25px rgba(0,0,0,0.1); }
                                                        .header { background: var(--user-color); height: 100px; }
                                                        .avatar-section { margin-top: -50px; text-align: center; }
                                                        .avatar { width: 100px; height: 100px; background: #fff; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; font-size: 2rem; border: 5px solid white; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
                                                        .content { padding: 20px 40px; }
                                                        .name-heading { text-align: center; margin-bottom: 30px; }
                                                        .name-heading h1 { margin: 0; color: #333; }
                                                        .name-heading p { margin: 5px 0; color: #777; font-style: italic; }
                                                        .info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; border-top: 1px solid #eee; padding-top: 20px; }
                                                        .info-item label { display: block; font-size: 0.8rem; color: #999; text-transform: uppercase; letter-spacing: 1px; }
                                                        .info-item span { font-weight: 600; color: #444; }
                                                        .hobby-tag { display: inline-block; background: #eee; padding: 2px 10px; border-radius: 20px; font-size: 0.9rem; margin-top: 5px; }
                                                    </style>
                                                </head>
                                                <body>
                                                    <div class='profile-card'>
                                                        <div class='header'></div>
                                                        <div class='avatar-section'>
                                                            <div class='avatar'>👤</div>
                                                        </div>
                                                        <div class='content'>
                                                            <div class='name-heading'>
                                                                <h1>{{NAME}} {{SURNAME}}</h1>
                                                                <p>{{OCCUPATION}}</p>
                                                            </div>
                                                            <div class='info-grid'>
                                                                <div class='info-item'>
                                                                    <label>Age</label>
                                                                    <span>{{AGE}} years old</span>
                                                                </div>
                                                                <div class='info-item'>
                                                                    <label>Favorite Color</label>
                                                                    <span style='color: var(--user-color)'>● {{FAVOURITE_COLOR}}</span>
                                                                </div>
                                                                <div class='info-item' style='grid-column: span 2;'>
                                                                    <label>Hobby</label>
                                                                    <div class='hobby-tag'>{{HOBBY}}</div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </body>
                                                </html>
                                        """;
}