describe('API - Adicionar Produto ao Carrinho', () => {
    it('deve autenticar, pegar um produto e adicionar ao carrinho', () => {
      cy.loginAPI().then((token) => {
        // Primeiro pega a lista de produtos
        cy.request({
          method: 'GET',
          url: 'https://localhost:8081/api/products',
          headers: { Authorization: `Bearer ${token}` }
        }).then((resProdutos) => {
          expect(resProdutos.status).to.eq(200);
          const produto = resProdutos.body.data[0]; // usa o primeiro produto da lista
  
          // Monta corpo para adicionar ao carrinho
          const carrinho = {
            date: new Date().toISOString(),
            userId: 1, // use o ID real do usuário logado se necessário
            products: [
              {
                productId: produto.id,
                quantity: 3,
                unitPrice: produto.price
              }
            ]
          };
  
          // Adiciona ao carrinho
          cy.request({
            method: 'POST',
            url: 'https://localhost:8081/api/carts',
            headers: {
              Authorization: `Bearer ${token}`
            },
            body: carrinho
          }).then((resCarrinho) => {
            expect(resCarrinho.status).to.be.oneOf([200, 201]);
            expect(resCarrinho.body.data).to.have.property('id');
          });
        });
      });
    });
  });
  