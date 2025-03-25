
describe('API - Login', () => {
  it('deve retornar um token ao autenticar', () => {
    cy.request('POST', 'https://localhost:8081/api/auth', {
      email: 'admin@example.com',
      password: 'A#g7jfdsd#$%#'
    }).then((res) => {
      expect(res.status).to.eq(200);
      expect(res.body.data).to.have.property('token');
    });
  });
});
