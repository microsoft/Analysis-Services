import { AppPage } from './app.po';

describe('gridcontrol App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display welcome message', async () => {
    page.navigateTo();
    const paragraphText = await page.getParagraphText();
    expect(paragraphText).toEqual('Welcome to app!');
  });
});